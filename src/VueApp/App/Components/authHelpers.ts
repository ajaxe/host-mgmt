import Vue from "vue";
import { Route, RawLocation } from "vue-router";
import { Api } from "./api";
import { RouteNames } from "../routes";

export const AuthHelpers = {
    checkRouteAuthorization(to: Route,
        from: Route,
        next: (to?: RawLocation | false | ((vm: Vue) => any) | void) => void): void {
        let api: Api = new Api();
        api.checkUserSession()
            .then(function (isValid) {
                if (isValid) {
                    next();
                }
                else {
                    next({ name: RouteNames.Login });
                }
            })
            .catch(function () {
                next({ name: RouteNames.Login });
            });
    }
}