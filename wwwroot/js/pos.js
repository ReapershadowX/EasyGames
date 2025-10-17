// wwwroot/js/pos.js

// Created with the Help of AI Language Model (ChatGPT)

$(function () {
    // Cart array to hold line items
    let cart = [];

    // Recalculate totals and update UI
    function updateTotals() {
        let total = 0;
        cart.forEach(item => total += item.subtotal);
        $('#totalBefore').text(total.toFixed(2));
        const tierRate = parseFloat($('#TierDiscount').val() || 0) / 100;
        const discount = total * tierRate;
        $('#totalDiscount').text(discount.toFixed(2));
        $('#grandTotal').text((total - discount).toFixed(2));
    }

    // Render the cart table body
    function renderCart() {
        const $tbody = $('#cartTable tbody').empty();
        cart.forEach(item => {
            const row = `
        <tr data-stock-id="${item.stockId}">
          <td>${item.name}</td>
          <td class="text-center">${item.qty}</td>
          <td class="text-end">${item.unitPrice.toFixed(2)}</td>
          <td class="text-end">${item.subtotal.toFixed(2)}</td>
          <td class="text-center">
            <button type="button" class="btn btn-sm btn-danger remove-item">×</button>
          </td>
        </tr>`;
            $tbody.append(row);
        });
        updateTotals();
    }

    // Lookup customer by phone
    $('#lookupBtn').click(function () {
        const phone = $('#CustomerPhone').val();
        const shopId = new URLSearchParams(window.location.search).get('shopId');
        if (!phone) return alert('Enter a phone number');
        $.post(`/POS/LookupCustomer`, { phone, shopId })
            .done(res => {
                if (res.success) {
                    $('#CustomerName').val(res.customerName);
                    $('#TierDiscount').val(res.tierDiscount);
                    updateTotals();
                } else {
                    alert(res.message || 'Customer not found');
                }
            })
            .fail(() => alert('Lookup failed'));
    });

    // Add item to cart
    $('#addBtn').click(function () {
        const stockId = parseInt($('#SelectedStockId').val());
        const qty = parseInt($('#Quantity').val());
        const shopId = new URLSearchParams(window.location.search).get('shopId');
        if (!stockId || qty <= 0) return alert('Select item and quantity');
        $.post(`/POS/AddToCart`, { stockId, quantity: qty, shopId })
            .done(res => {
                if (res.success) {
                    cart.push({
                        stockId,
                        name: res.itemName,
                        qty,
                        unitPrice: res.unitPrice,
                        subtotal: res.subtotal
                    });
                    renderCart();
                } else {
                    alert(res.message);
                }
            })
            .fail(() => alert('Add to cart failed'));
    });

    // Remove item from cart
    $('#cartTable').on('click', '.remove-item', function () {
        const stockId = parseInt($(this).closest('tr').data('stock-id'));
        cart = cart.filter(i => i.stockId !== stockId);
        renderCart();
    });

    // Before submitting the sale form, serialize cart into hidden field
    $('#posForm').submit(function () {
        if (cart.length === 0) {
            alert('Cart is empty');
            return false;
        }
        $('#CartJson').val(JSON.stringify(cart));
        return true;
    });
});
