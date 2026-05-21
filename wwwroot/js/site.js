// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Add loading state to forms when submitted
document.addEventListener('DOMContentLoaded', function () {
    const forms = document.querySelectorAll('form');
    
    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            // Check if form is valid using HTML5 / jQuery Validation
            if (this.checkValidity() && (!window.jQuery || !$(this).data('validator') || $(this).valid())) {
                const submitBtn = this.querySelector('button[type="submit"]');
                if (submitBtn) {
                    submitBtn.classList.add('btn-loading');
                }
            }
        });
    });
});
