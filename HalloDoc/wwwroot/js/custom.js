function darkMode() {
    var element = document.querySelector(".patient-login-main");
   
    element.classList.toggle("dark-mode");
   
}
const eyeIcon = document.querySelector('.eye-icon');
const password = document.querySelector('.password');
eyeIcon.addEventListener("click", function(){
    const type = password.getAttribute("type") === "password" ? "text" : "password";
            password.setAttribute("type", type);
            
            // toggle the icon
            this.classList.toggle("fa-regular fa-eye");
});
function darkModeReset() {
    var element = document.querySelector(".patient-reset-main");
   
    element.classList.toggle("dark-mode");
   
}
function darkModePatientSitePage(){
     var element1 = document.querySelector(".patient-site-main");
     element1.classList.toggle("dark-mode");
}

function darkModePatientRequestPage(){
    var element1 = document.querySelector(".patient-submit-main");
    element1.classList.toggle("dark-mode");
}

function darkModeCreatePatientRequestPage(){
    var element1 = document.querySelector(".create-patient-req-main");
    element1.classList.toggle("dark-mode");
}



