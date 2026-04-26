
$(document).ready(function () {
    function showToast(title, message, type = 'success') {
        //if toast container doesn't exist, create it
        let toastContainer = $('.toast-container');
        if (toastContainer.length === 0) {
            $('body').append('<div class="toast-container position-fixed bottom-0 end-0 p-3" style="z-index: 1100"></div>');
            toastContainer = $('.toast-container');
        }

        const colors = {
            success: { bg: '#2a5f3a', border: '#4caf50', icon: 'bi-check-circle-fill' },
            error: { bg: '#5f2a2a', border: '#dc3545', icon: 'bi-exclamation-triangle-fill' },
            warning: { bg: '#5f4a2a', border: '#ffc107', icon: 'bi-exclamation-triangle-fill' },
            info: { bg: '#2a4a5f', border: '#17a2b8', icon: 'bi-info-circle-fill' }
        };

        const color = colors[type] || colors.success;

        const toastHtml = `
            <div class="toast align-items-center text-white border-0 show mb-2" role="alert" style="background: linear-gradient(135deg, ${color.bg}, #1a1410); border-left: 4px solid ${color.border}; min-width: 300px; border-radius: 10px; backdrop-filter: blur(10px);">
                <div class="toast-header" style="background: ${color.bg}; color: #ffd6b0; border-bottom: 1px solid rgba(255,107,53,0.2);">
                    <i class="bi ${color.icon} me-2" style="color: ${color.border};"></i>
                    <strong class="me-auto">${title}</strong>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>
                </div>
                <div class="toast-body" style="color: #ffd6b0;">
                    <i class="bi bi-egg-fried me-2" style="color: ${color.border};"></i>
                    ${message}
                </div>
            </div>
        `;

        const toastElement = $(toastHtml);
        toastContainer.append(toastElement);

        // auto-hide after 5 seconds
        setTimeout(() => {
            toastElement.fadeOut(500, function () {
                $(this).remove();
            });
        }, 5000);
    }

    //directly expose showToast to global scope for server-side use
    window.showToast = showToast;

    // reads temp data from hidden inputs and shows toasts on page load,
    // then clears the temp data
    const success = $('#tempDataSuccess').val();
    const error = $('#tempDataError').val();
    const warning = $('#tempDataWarning').val();
    const info = $('#tempDataInfo').val();

    if (success && success !== '' && success !== 'null') {
        showToast('🍞 Success!', success, 'success');
        $('#tempDataSuccess').val(''); 
    }
    if (error && error !== '' && error !== 'null') {
        showToast('❌ Error!', error, 'error');
        $('#tempDataError').val('');
    }
    if (warning && warning !== '' && warning !== 'null') {
        showToast('⚠️ Warning', warning, 'warning');
        $('#tempDataWarning').val('');
    }
    if (info && info !== '' && info !== 'null') {
        showToast('ℹ️ Info', info, 'info');
        $('#tempDataInfo').val('');
    }

    // global AJAX listener for toast messages from server responses

    $(document).ajaxComplete(function (event, xhr, settings) {
        const toast = xhr.getResponseHeader('X-Toast-Message');
        const toastType = xhr.getResponseHeader('X-Toast-Type') || 'success';

        if (toast) {
            showToast('🍞 Gruer\'s', toast, toastType);
        }
    });
});