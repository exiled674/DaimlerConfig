// wwwroot/JSFolder/toast.js
window.showSuccessToast = () => {
    const toastEl = document.getElementById('successToast');
    if (toastEl) {
        const toast = new bootstrap.Toast(toastEl);
        toast.show();
    }
}

window.showErrorToast = () => {
    const toastEl = document.getElementById('errorToast');
    if (toastEl) {
        const toast = new bootstrap.Toast(toastEl);
        toast.show();
    }
};

// New functions for showing toasts with custom messages
window.showSuccessToastWithMessage = async (dotNetHelper, message) => {
    await dotNetHelper.invokeMethodAsync('ShowSuccess', message);
}

window.showErrorToastWithMessage = async (dotNetHelper, message) => {
    await dotNetHelper.invokeMethodAsync('ShowError', message);
}