<template>
  <div>
    <form
      method="post"
      :action="googleLoginUrl"
      v-on:submit = "loginSubmit" >
      <input
        type="hidden"
        name="__RequestVerificationToken" v-model="requestVerificationToken" />
      <input
        type="hidden"
        name="LoginType" v-model="loginType" />
      <input
        type="hidden"
        name="ReturnUrl" v-model="returnUrl" />
      <button
        type="submit"
        class="btn btn-outline-secondary"
        v-on:click="googleClick">Login using Google</button>
    </form>
  </div>
</template>

<script lang="ts">
import { Vue, Component, Prop, Provide } from "vue-property-decorator";
import { Api } from "./api";
import { LoginData } from "./Types/loginData";
import { EventNames } from "../events";

declare var gapi: any;

@Component({
  mounted: function() {
    this.$eventBus.$emit(EventNames.LoginFailure);
    /*gapi.signin2.render('google-signin-button', {
      onsuccess: this.onGoogleSignIn
    })*/
  }
})
export default class Login extends Vue {
  readonly api: Api = new Api();
  @Provide() requestVerificationToken: string = this.api.getRequestHeaderToken();
  @Provide() returnUrl: string = Api.HomeIndexUrl;
  @Provide() loginType: string = "Google";
  @Provide() googleLoginUrl: string  = Api.GoogleLoginUrl;

  googleClick(event: Event) {
      console.log("googleClick");
      this.requestVerificationToken = this.api.getRequestHeaderToken();
      this.loginType = "Google";
      this.returnUrl = "/home/index";
  }

  onGoogleSignIn(googleUser: any) {
    var profile = googleUser.getBasicProfile();
    console.log('ID: ' + profile.getId()); // Do not send to your backend! Use an ID token instead.
    console.log('Name: ' + profile.getName());
    console.log('Image URL: ' + profile.getImageUrl());
    console.log('Email: ' + profile.getEmail());
  }

  loginSubmit(event: Event) {
      console.log(`loginSubmit: ${this.requestVerificationToken}|${this.loginType}|${this.returnUrl}`);
  }
}
</script>
