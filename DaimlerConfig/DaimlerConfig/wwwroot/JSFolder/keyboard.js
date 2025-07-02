window.keyboardHandler = {
    dotNetRef: null,
    boundHandleKeyDown: null,  // hier speichern wir die gebundene Funktion

    addKeyListener: function (dotNetRef) {
        this.dotNetRef = dotNetRef;
        if (!this.boundHandleKeyDown) {
            this.boundHandleKeyDown = this.handleKeyDown.bind(this);
            document.addEventListener('keydown', this.boundHandleKeyDown);
            console.log("Keyboard listener added");
        }
    },

    handleKeyDown: function (e) {
        if (e.ctrlKey && e.key === 'c') {
            this.dotNetRef.invokeMethodAsync('HandleCtrlC').then(handled => {
                if (handled) {
                    e.preventDefault();
                }
            });
        }
        else if (e.ctrlKey && e.key === 'v') {
            this.dotNetRef.invokeMethodAsync('HandleCtrlV').then(handled => {
                if (handled) {
                    e.preventDefault();
                }
            });
        }
        else if (e.key === 'Delete') {
            this.dotNetRef.invokeMethodAsync('HandleDelete').then(handled => {
                if (handled) {
                    e.preventDefault();
                }
            });
        }
        else if (e.key === 'Escape') {
            this.dotNetRef.invokeMethodAsync('HandleEscape');
        }
    },

    removeKeyListener: function () {
        if (this.dotNetRef && this.boundHandleKeyDown) {
            document.removeEventListener('keydown', this.boundHandleKeyDown);
            this.boundHandleKeyDown = null;
            this.dotNetRef = null;
            console.log("Keyboard listener removed");
        }
    }
};
