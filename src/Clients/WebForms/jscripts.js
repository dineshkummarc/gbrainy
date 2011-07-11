toggleVisibleById = function (elementId) {
    toggleVisible(document.getElementById(elementId));
};

toggleVisible = function (element) {
    element.style.display == 'none' ? 'block' : 'none';
};

