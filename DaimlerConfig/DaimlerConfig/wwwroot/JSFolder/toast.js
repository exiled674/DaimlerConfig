// wwwroot/JSFolder/toast.js

window.showSuccessToast = () => {
    const toastEl = document.getElementById('successToast');
    if (toastEl) {
        const toast = new bootstrap.Toast(toastEl);
        toast.show();
    }
}
