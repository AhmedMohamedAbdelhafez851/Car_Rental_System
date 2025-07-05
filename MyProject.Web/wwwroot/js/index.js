/**
 * JavaScript لصفحة قائمة الحجوزات
 */
(function () {
    // وظيفة debounce لتقليل عدد طلبات AJAX
    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    // البحث في الحجوزات
    function searchRentals() {
        const query = $('#searchQuery').val().trim();
        const $spinner = $('.loading-spinner');
        $spinner.show();

        $.ajax({
            url: '@Url.Action("Index")',
            data: { searchQuery: query },
            success: function (data) {
                const newTableBody = $(data).find('#rentalsTable').html();
                $('#rentalsTable').html(newTableBody);
                $spinner.hide();
            },
            error: function () {
                $spinner.hide();
                showError('حدث خطأ أثناء البحث');
            }
        });
    }

    // عرض رسائل النجاح أو الخطأ
    function showSuccess(message) {
        const alert = `<div class="alert alert-success alert-dismissible fade show animate__animated animate__fadeIn" role="alert">
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>`;
        $('body').append('<div class="position-fixed top-0 end-0 p-3" style="z-index: 1100"></div>').find('.position-fixed').html(alert);
        setTimeout(() => $('.alert').alert('close'), 5000);
    }

    function showError(message) {
        const alert = `<div class="alert alert-danger alert-dismissible fade show animate__animated animate__fadeIn" role="alert">
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>`;
        $('body').append('<div class="position-fixed top-0 end-0 p-3" style="z-index: 1100"></div>').find('.position-fixed').html(alert);
        setTimeout(() => $('.alert').alert('close'), 5000);
    }

    // تهيئة الصفحة
    $(document).ready(function () {
        // إضافة حدث البحث مع debounce
        $('#searchQuery').on('input', debounce(searchRentals, 300));

        // عرض رسائل TempData
        @if (TempData["SuccessMessage"] != null) {
            <text>showSuccess('@TempData["SuccessMessage"]');</text>
        }
        @if (TempData["ErrorMessage"] != null) {
            <text>showError('@TempData["ErrorMessage"]');</text>
        }
    });
})();