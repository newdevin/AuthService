namespace AuthService
open Saturn

module Router = 

    let apiRouter = router {
        getf "/api/%s" (fun name ->  Service.getToken name)
    }

