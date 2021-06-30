
  <script src="https://www.gstatic.com/firebasejs/live/3.0/firebase.js"></script>
var config = {
        apiKey: "AIzaSyDuN6ePGAyWeRJhb4q5SLsdXADXNAxrkgE",
        authDomain: "cardgameweb.firebaseapp.com",
        databaseURL: "https://cardgameweb-default-rtdb.firebaseio.com",
        projectId: "cardgameweb",
        storageBucket: "cardgameweb.appspot.com",
        messagingSenderId: "91434336416",
        appId: "1:91434336416:web:4d963a06be7eb46a6589c5",
        measurementId: "G-7V9K1D2B76"
    };
    firebase.initializeApp(config);
    Signed in
    firebase.auth().signInWithEmailAndPassword("email@domain.com", "password").then((userCredential) => {
    
    var user = firebase.auth().currentUser;
    window.alert(user.uid);
    // ...
    }).catch(function(error){
        var errorcode = error.code;
        var errorMessage = error.message;
        console.log('SignIn error',error);
    });
    function RegisterAndCreateRecord()
    {
    //Register
    firebase.auth().createUserWithEmailAndPassword("zohaibnasir008@gmail.com", "password").then((userCredential) => {
    var database = firebase.database();
    var user = firebase.auth().currentUser;
    //window.alert(user.uid);
    var userId = user.uid;
    var name = "GreenTiger";
    var wins = 0;
    var email = "zohaibnasir045@gmail.com";
    document.getElementById("ReturnString").innerHTML = name;
            //create user data in realtime database
            firebase.database().ref('users/' + userId).set({
                username: name,
                email: email,
                totalWins : wins
            });
    // ...
    }).catch(function(error){
        var errorcode = error.code;
        var errorMessage = error.message;
        console.log('Register error error',error);
    });
    }