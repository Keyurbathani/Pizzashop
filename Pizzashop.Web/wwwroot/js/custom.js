
var password = document.getElementById("floatingPassword");
let eye = document.getElementById("inputGroupPrepend");

 function eyeShow() {

    if ( password.type === "password") {
        password.type = "text";
        eye.innerHTML=`<i class="fa-solid fa-eye"></i>`
    }

    else {
        password.type = "password";
        eye.innerHTML=`<i class="fa-regular fa-eye-slash"></i>`
    }

}

var passwordOne = document.getElementById("floatingPasswordOne");
var eyeOne = document.getElementById("inputGroupPrepend1");

 function eyeShow2() {

    if (passwordOne.type === "password") {
        passwordOne.type = "text";
         eyeOne.innerHTML=`<i class="fa-solid fa-eye"></i>`
    }

    else {
        passwordOne.type = "password";
        eyeOne.innerHTML=`<i class="fa-regular fa-eye-slash"></i>`
    }
}

var passwordTwo = document.getElementById("floatingPasswordTwo");
var eyeTwo = document.getElementById("inputGroupPrepend2");

 function eyeShow3() {

    if (passwordTwo.type === "password") {
        passwordTwo.type = "text";
         eyeTwo.innerHTML=`<i class="fa-solid fa-eye"></i>`
    }

    else {
        passwordTwo.type = "password";
         eyeTwo.innerHTML=`<i class="fa-regular fa-eye-slash"></i>`
    }

}



