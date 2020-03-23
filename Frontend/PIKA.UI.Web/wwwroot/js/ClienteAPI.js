var ClienteAPI = (function () {

    // Keep this variable private inside this closure scope
    var myGrades = [93, 95, 88, 0, 55, 91];

    var average = function () {
        var total = myGrades.reduce(function (accumulator, item) {
            return accumulator + item;
        }, 0);

        return 'Your average grade is ' + total / myGrades.length + '.';
    };

    var failing = function () {
        var failingGrades = myGrades.filter(function (item) {
            return item < 70;
        });

        return 'You failed ' + failingGrades.length + ' times.';
    };

    var jwt = function () {
        return "aaaaa";
        mgr.getUser().then(function (user) {
            if (user != null) {
                console.log("0");   

                return user.access_token;
            } else {
                console.log("1");   
                return null;
            }

        }).catch(function (err) {
            console.log("2");   
            return null;
        });

    }

    return {
        average: average,
        failing: failing,
        jwt: jwt
    }
})();
