function addServiceInput() {
    var inputFieldsContainer = document.getElementById("servicesInputFieldContainer");
    var inputFieldSample = inputFieldsContainer.firstElementChild.cloneNode(true);
    if (inputFieldSample == null) {
        return;
    }
    inputFieldsContainer.appendChild(inputFieldSample);
}