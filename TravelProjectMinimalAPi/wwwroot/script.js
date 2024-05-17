document.getElementById("form1").addEventListener('submit', (e) => {
    e.preventDefault();

    console.log("da5alt")
    const destinationInput = document.getElementById("destinationInput")
    const fileInput = document.getElementById("fileInput")

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


    fetch("/add-destination", {
        method: "POST",
        body: formData
    })
        .then(response => response.json())
        .then(data => console.log(data))
        .catch(error => console.error("Error:", error))
})
