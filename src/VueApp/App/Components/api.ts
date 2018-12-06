import { LoginData } from "./Types/loginData";
import $ from 'jquery';

type AjaxHeaders = { [key: string]: string };
type HeaderOptions = { includeAuth?: boolean, includeCsrf?: boolean };

const baseUrl = $("#base-url").prop("href");
const RequestVerificationTokenName = "__RequestVerificationToken";
const getUserProfileUrl = `${baseUrl}api/Account/UserProfile`;
const postExternalLoginUrl = `${baseUrl}api/Account/ExternalLogin`;
const logoutUrl = `${baseUrl}api/Account/Signout`;
let requestToken: string = '';
export class Api {

    public getRequestHeaderToken(): string {
        if (!requestToken) {
            requestToken = <string>$('input[name=__RequestVerificationToken]:first').val();
        }
        return requestToken;
    }

    private getDataWithRequestVerificationToken(): any {
        let data = {};
        data[RequestVerificationTokenName] = this.getRequestHeaderToken();
        return data;
    }

    checkUserSession(): Promise<boolean> {
        return new Promise<boolean>(function (resolve/*, reject*/) {
            //resolve(true); return;
            $.get({
                url: getUserProfileUrl
            })
            .then(function () {
                resolve(true);
            })
            .catch(function () {
                console.log(arguments);
                resolve(false);
            });
        });
    }

    login(loginData: LoginData): Promise<string | null> {
        let self = this;
        return new Promise(function (resolve, reject) {
            let d = self.getDataWithRequestVerificationToken();
            Object.assign(d, loginData);
            $.post({
                url: postExternalLoginUrl,
                data: d,
                statusCode: {
                    200: function () {
                        resolve();
                    },
                    302: function() {
                        console.log('302 response');
                        console.log(arguments);
                        reject();
                    }
                }
            })
            .then(function () {
                resolve();
            })
            .catch(function () {
                console.log('Post error: login:');
                console.log(arguments);
                reject();
            });
        });
    }

    logout(): Promise<any> {
        let self = this;
        return new Promise<any>(function(resolve/*, reject*/) {
            $.post({
                url: logoutUrl,
                data: self.getDataWithRequestVerificationToken()
            })
            .then(function(){
                resolve();
            })
            .catch(function(){
                console.log(arguments);
                resolve();
            })
        });
    }

}