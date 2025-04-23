function showMinicartPopup() {
    const popup = $('#minicart-popup');
    popup.fadeIn(300);
    setTimeout(() => {
        popup.fadeOut(400);
    }, 3000);
    popup.find('.btn-close').on('click', function () {
        popup.fadeOut(200);
    });
}

$(document).ready(function () {
    $('.add-to-cart-btn').on('click', function (e) {
        e.preventDefault(); // 🛑 stoppar eventuell annan påverkan

        const productId = $(this).data('product-id');

        $.ajax({
            url: '/Cart/AddToCart',
            type: 'POST',
            data: { productId: productId },
            success: function (response) {
                $('#cart-count').text(response.totalItems); // 🔢 uppdaterar räkningen
                showMinicartPopup();
            },
            error: function () {
                alert('Något gick fel.');
            }
        });
    });
});
