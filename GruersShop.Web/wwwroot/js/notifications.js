// Simplified version with auto-hide only
$(document).ready(function () {
    let notificationTimeout;

    function checkForOrderUpdates() {
        if ($('.btn-warm:contains("Account")').length === 0) return;

        fetch('/api/orders/recent-updates')
            .then(response => response.json())
            .then(data => {
                if (data.hasUpdates) {
                    showNotification();
                }
            })
            .catch(error => console.log('Error:', error));
    }

    function showNotification() {
        // Clear any existing timeout
        if (notificationTimeout) clearTimeout(notificationTimeout);

        // Add badges
        const myOrdersLink = $('a[href*="/Orders/MyOrders"]');
        if (myOrdersLink.length && !myOrdersLink.find('.update-badge').length) {
            myOrdersLink.find('span:first').append('<span class="update-badge ms-2">🔔 New!</span>');
        }

        const accountBtn = $('.btn-warm.dropdown-toggle');
        if (accountBtn.find('.notification-dot').length === 0) {
            accountBtn.css('position', 'relative');
            accountBtn.append('<span class="notification-dot"></span>');
        }

        // Show toast
        $('.order-toast').remove();
       
        const toast = `
    <div class="order-toast position-fixed bottom-0 end-0 p-3" style="z-index: 9999">
        <div class="toast show" role="alert" style="background: linear-gradient(135deg, #2a1f1a 0%, #1a1410 100%); border-left: 4px solid #ff6b35; border-radius: 12px;">
            <div class="toast-header" style="background: #2a1f1a; color: #ffd6b0; border-bottom-color: rgba(255,107,53,0.2);">
                <i class="bi bi-bell-fill me-2" style="color: #ff6b35;"></i>
                <strong class="me-auto">🍞 Order Update</strong>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>
            </div>
            <div class="toast-body">
                <i class="bi bi-info-circle-fill me-2" style="color: #ff6b35;"></i>
                Your order status has been updated!
                <div class="mt-3">
                    <a href="/Orders/MyOrders" class="btn-warm-sm">
                        <i class="bi bi-eye me-1"></i> View My Orders
                    </a>
                </div>
            </div>
        </div>
    </div>
`;
        $('body').append(toast);

        // Auto-hide EVERYTHING after 10 seconds
        notificationTimeout = setTimeout(() => {
            $('.update-badge, .notification-dot, .order-toast').fadeOut(500, function () {
                $(this).remove();
            });
        }, 10000);
    }

    // Clear when clicking My Orders
    $(document).on('click', 'a[href*="/Orders/MyOrders"]', function () {
        if (notificationTimeout) clearTimeout(notificationTimeout);
        $('.update-badge, .notification-dot, .order-toast').remove();
    });

    setInterval(checkForOrderUpdates, 30000);
    setTimeout(checkForOrderUpdates, 1000);
});