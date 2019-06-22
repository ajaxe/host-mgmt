import { UserProfile } from "./Types/userProfile";
import $ from 'jquery';
import { ApiCredential } from "./Types/apiCredential";
import { NewApiKey } from "./Types/newApiKey";

type AjaxHeaders = { [key: string]: string };
type HeaderOptions = { includeAuth?: boolean, includeCsrf?: boolean };

const baseUrl = document.baseURI;
const RequestVerificationTokenName = "__RequestVerificationToken";
const getUserProfileUrl = `${baseUrl}api/Account/UserProfile`;
const logoutUrl = `${baseUrl}api/Account/Signout`;
const deleteUser = `${baseUrl}api/Account`;
const getApiKeysDisplayUrl = `${baseUrl}api/ApiKeys/Display`;
const postApiKeyCreateUrl = `${baseUrl}api/ApiKeys/Create`;
const getApiKeyByIdUrl = `${baseUrl}api/ApiKeys`;
const deleteApiKeyByIdUrl = `${baseUrl}api/ApiKeys`;
const postGoogleOAuthLogin = `${baseUrl}api/Account/GoogleOAuthLogin`;

export class Api {
    static requestToken: string = '';
    static userProfile: UserProfile = null;
    static GoogleLoginUrl: string = postGoogleOAuthLogin;
    static HomeIndexUrl: string = `${baseUrl}home/index`;

    public getRequestHeaderToken(): string {
        if (!Api.requestToken) {
            Api.requestToken = <string>$('input[name=__RequestVerificationToken]:first').val();
        }
        return Api.requestToken;
    }

    private getDataWithRequestVerificationToken(): any {
        let data = {};
        data[RequestVerificationTokenName] = this.getRequestHeaderToken();
        return data;
    }

    getUserProfile(): UserProfile {
        return Api.userProfile;
    }

    checkUserSession(): Promise<boolean> {
        return new Promise<boolean>(function (resolve/*, reject*/) {
            //resolve(true); return;
            $.get({
                url: getUserProfileUrl
            })
            .then(function (data: any) {
                Api.userProfile = <UserProfile>data;
                resolve(true);
            })
            .catch(function () {
                Api.userProfile = null;
                console.log(arguments);
                resolve(false);
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

    deleteUser(): Promise<any> {
        let profile = this.getUserProfile();
        return new Promise<any>(function(resolve, reject) {
            $.ajax({
                method: "DELETE",
                url: `${deleteUser}/${profile.externalId}`
            })
            .then(function() {
                resolve();
            })
            .catch(function() {
                console.log(arguments);
                reject();
            })
        });
    }

    getApiKeys(): Promise<ApiCredential[]> {
        return new Promise<ApiCredential[]>(function(resolve, reject){
            $.get(getApiKeysDisplayUrl)
            .then(function(data){
                resolve(data);
            })
            .catch(function() {
                reject();
            });
        });
    }

    createApiKey(): Promise<NewApiKey> {
        let self = this;
        return new Promise<NewApiKey>(function(resolve, reject) {
            $.post({
                url: postApiKeyCreateUrl,
                data: self.getDataWithRequestVerificationToken()
            })
            .then(function(data) {
                resolve(<NewApiKey>data);
            })
            .catch(function(){
                reject();
            });
        });
    }

    getApiKeyById(id: number): Promise<ApiCredential> {
        return new Promise<ApiCredential>(function(resolve, reject) {
            $.get(`${getApiKeyByIdUrl}/${id}`)
            .then(function(data) {
                resolve(<ApiCredential>data);
            })
            .catch(function() {
                reject();
            });
        });
    }

    deleteApiKeyById(id: number): Promise<ApiCredential> {
        let self = this;
        return new Promise<ApiCredential>(function(resolve, reject) {
            $.ajax({
                method: "DELETE",
                url: `${deleteApiKeyByIdUrl}/${id}`,
                data: self.getDataWithRequestVerificationToken()
            })
            .then(function(data) {
                resolve(<ApiCredential>data);
            })
            .catch(function() {
                reject();
            });
        });
    }
}