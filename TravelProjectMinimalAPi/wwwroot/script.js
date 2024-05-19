document.getElementById("form1").addEventListener('submit', (e) => {
    e.preventDefault();

    const destinationInput = document.getElementById("destinationInput")
    const fileInput = document.getElementById("fileInput")
    const ratingInput = document.getElementById("rating")

    const destinationValidation = document.getElementById("destination-validation")
    const fileValidation = document.getElementById("file-validation")

    destinationValidation.innerText = ""
    fileValidation.innerText = ""

    var isValid = true

    if (destinationInput.value === "") {
        destinationValidation.innerText = "Destination is required"
        isValid = false
    }
    if (fileInput.value === "" || !/\.(jpg|jpeg|png|gif)$/.test(fileInput.value)) {
        fileValidation.innerText = "Please select a file"
        isValid = false
    }
    if (!isValid) { return }

    var formData = new FormData();
    formData.append("destination", destinationInput.value)
    formData.append("file", fileInput.files[0], fileInput.files[0].name)
    formData.append("rating", ratingInput.value)

    destinationInput.value = ""
    ratingInput.value = 5

    fetch("/add-destination", {
        method: "POST",
        body: formData
    })
        .then(() => { })
        .catch(error => console.error("Error:", error))
})

document.getElementById("form2").addEventListener('submit', (e) => {
    e.preventDefault();

    const deleteInput = document.getElementById("delete-input")

    const deleteValidation = document.getElementById("delete-validation")

    deleteValidation.innerText = ""

    var isValid = true

    if (deleteInput.value === "") {
        destinationValidation.innerText = "Destination name is required"
        isValid = false
    }
    if (!isValid) { return }


    fetch(`/remove-destination/${deleteInput.value}`, {
        method: "DELETE",
    })
        .then(() => {deleteInput.value = "" })
        .catch(error => console.error("Error:", error))
})

