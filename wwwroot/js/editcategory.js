function addServiceInput() {
    var inputFieldsContainer = document.getElementById("servicesInputFieldContainer");
    var oldInputFiledsContainer = document.getElementById("oldServicesInputFieldsContainer");
    var oldCount = oldInputFiledsContainer.children.length;
    var newCount = inputFieldsContainer.children.length;
    var inputsCount = oldCount + newCount;
    //alert("old:" + oldCount + " new:" + newCount + " sum:" + inputsCount);
    var inputFieldClone = inputFieldsContainer.firstElementChild.cloneNode(true);
    inputFieldClone.firstElementChild.setAttribute("name", "Services[" + inputsCount + "].Name");
    if (inputFieldClone == null) {
        return;
    }
    inputFieldsContainer.appendChild(inputFieldClone);
}