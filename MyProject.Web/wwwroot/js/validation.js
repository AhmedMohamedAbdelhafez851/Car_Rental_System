/**
 * validation.js
 * يحتوي على دوال التحقق من الصحة وعرض الرسائل
 */
(function () {
    function showSuccessMessage(message) {
        const alert = document.createElement("div");
        alert.className = "alert alert-success alert-dismissible fade show position-fixed";
        alert.style.cssText = "top: 20px; right: 20px; z-index: 9999; min-width: 300px;";
        alert.innerHTML = `
            <i class="fas fa-check-circle me-2"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        document.body.appendChild(alert);
        setTimeout(() => {
            if (alert.parentNode) alert.remove();
        }, 3000);
    }

    function showErrorMessage(message) {
        const alert = document.createElement("div");
        alert.className = "alert alert-danger alert-dismissible fade show position-fixed";
        alert.style.cssText = "top: 20px; right: 20px; z-index: 9999; min-width: 300px;";
        alert.innerHTML = `
            <i class="fas fa-exclamation-triangle me-2"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        document.body.appendChild(alert);
        setTimeout(() => {
            if (alert.parentNode) alert.remove();
        }, 5000);
    }

    function validateField(event) {
        const field = event.target;
        if (field.hasAttribute("required") && !field.value.trim()) {
            field.classList.add("is-invalid");
            field.classList.remove("is-valid");
            const feedback = field.parentNode.querySelector(".invalid-feedback") || document.createElement("span");
            feedback.className = "invalid-feedback";
            feedback.textContent = `${field.previousElementSibling.textContent} مطلوب`;
            if (!field.parentNode.querySelector(".invalid-feedback")) {
                field.parentNode.appendChild(feedback);
            }
            feedback.style.display = "block";
        } else {
            field.classList.remove("is-invalid");
            field.classList.add("is-valid");
            const feedback = field.parentNode.querySelector(".invalid-feedback");
            if (feedback) feedback.style.display = "none";
        }
    }

    function clearFieldError(event) {
        event.target.classList.remove("is-invalid");
        const feedback = event.target.parentNode.querySelector(".invalid-feedback");
        if (feedback) feedback.style.display = "none";
    }

    window.validation = {
        showSuccessMessage,
        showErrorMessage,
        validateField,
        clearFieldError
    };
})();