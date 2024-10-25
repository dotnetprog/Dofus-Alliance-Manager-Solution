$(document).ready(function () {
    if (!window.JWTManager.getToken() || window.JWTManager.getToken() == "undefined" ) {
        fetch("/api/Login/Token").then((response) => {
            response.json().then((r) => {
                window.JWTManager.setToken(r.apiToken);
                window.JWTManager.monitor((token) => {
                    fetch("/api/Login/Token").then((resp) => {
                        resp.json().then((result) => {
                            window.JWTManager.refresh(result.apiToken);

                        })


                    }).catch(error => {
                        console.error(error);

                    });
                }, 30);

            });


        }).catch(error => {
            console.error(error);

        });
    } else {
        window.JWTManager.monitor((token) => {
            fetch("/api/Login/Token").then((resp) => {
                resp.json().then((result) => {
                    window.JWTManager.refresh(result.apiToken);

                })


            }).catch(error => {
                console.error(error);

            });
        }, 30);
    }
    
   
})