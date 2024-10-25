
class Local {
    store(token) {
        localStorage.setItem('jwt', token);
    }
    retrieve() {
        let result = localStorage.getItem('jwt');
        return result ? result : "";
    }
    forget() {
        localStorage.removeItem('jwt');
    }
}
class Token {
    constructor(decoded) {
        this.decoded = decoded;
        this.expiry = decoded.exp;
    }
    getPayload() {
        return this.decoded;
    }
    getExpiry() {
        return this.expiry;
    }
}
class Decoder {
    constructor() {
        this.store = new Cookie();
    }
    useLocalStore() {
        this.store = new Local();
    }
    decode() {
        let token = this.store.retrieve();
        if (token) {
            let sectioned = token.split('.')[1];
            let replaced = sectioned.replace('-', '+').replace('_', '/');
            let decoded = JSON.parse(window.atob(replaced));
            return new Token(decoded);
        }
        throw new TypeError("No token has been set");
    }
}
class JWTManager {
    constructor() {
        this.secondsInterval = 10;
        this.store = new Cookie();
        this.decoder = new Decoder();
    }
    setToken(token) {
        this.store.store(token);
    }
    getToken() {
        return this.store.retrieve();
    }
    forget() {
        this.store.forget();
    }
    refresh(token) {
        this.store.forget();
        this.store.store(token);
    }
    decode() {
        return this.decoder.decode();
    }
    monitor(callback, secondsLimit = 60) {
        setInterval(() => {
            try {
                let remainingSeconds = this.secondsRemaining();
                if (remainingSeconds <= secondsLimit) {
                    callback(this.getToken());
                }
            }
            catch (e) {
            }
        }, this.getSecondsInterval());
    }
    useLocalStore() {
        this.store = new Local();
        this.decoder.useLocalStore();
    }
    secondsRemaining() {
        let token = this.decode();
        if (token) {
            return token.getExpiry() - (Date.now() / 1000);
        }
    }
    hasTokenExpired() {
        let secondsRemaining = this.secondsRemaining();
        if (!secondsRemaining) {
            return false;
        }
        return secondsRemaining < 0;
    }
    getSecondsInterval() {
        return this.secondsInterval * 1000;
    }
}
class Cookie {
    store(token) {
        let date = this.getExpiryDate();
        document.cookie = `jwt=${token};expires=${date.toUTCString()};path=/`;
    }
    retrieve() {
        let key = 'jwt=';
        let decodedCookie = decodeURIComponent(document.cookie);
        let cookieArray = decodedCookie.split(';');
        let cookieArrayLength = cookieArray.length;
        for (let i = 0; i < cookieArrayLength; i++) {
            let cookie = cookieArray[i];
            while (cookie.charAt(0) == ' ') {
                cookie = cookie.substring(1);
            }
            if (cookie.indexOf(key) == 0) {
                return cookie.substring(key.length, cookie.length);
            }
        }
        return "";
    }
    forget() {
        document.cookie = 'jwt=; expires=Thu, 01-Jan-70 00:00:01 GMT;';
    }
    getExpiryDate() {
        let date = new Date();
        return new Date(date.getTime() + 86400000);
    }
}