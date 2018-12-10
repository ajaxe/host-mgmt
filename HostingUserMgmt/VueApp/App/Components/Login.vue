<template>
  <form
    method="post"
    :action="externalLoginUrl"
    v-on:submit = "loginSubmit"
  >
    <input
      type="hidden"
      name="__RequestVerificationToken" v-model="requestVerificationToken"
    />
    <input
      type="hidden"
      name="LoginType" v-model="loginType"
    />
    <input
      type="hidden"
      name="ReturnUrl" v-model="returnUrl"
    />
    <button
      type="submit"
      class="btn btn-outline-secondary"
      v-on:click="googleClick"
    >Login using Google</button>
  </form>
</template>

<script lang="ts">
import { Vue, Component, Prop, Provide } from "vue-property-decorator";
import { Api } from "./api";
import { LoginData } from "./Types/loginData";
import { EventNames } from "../events";

@Component({
  mounted: function() {
    this.$eventBus.$emit(EventNames.LoginFailure);
  }
})
export default class Login extends Vue {
  readonly api: Api = new Api();
  @Provide() requestVerificationToken: string = this.api.getRequestHeaderToken();
  @Provide() returnUrl: string = Api.HomeIndexUrl;
  @Provide() loginType: string = "Google";
  @Provide() externalLoginUrl: string  = Api.ExternalLoginUrl;

  googleClick(event: Event) {
      console.log("googleClick");
      this.requestVerificationToken = this.api.getRequestHeaderToken();
      this.loginType = "Google";
      this.returnUrl = "/home/index";
  }

  loginSubmit(event: Event) {
      console.log(`loginSubmit: ${this.requestVerificationToken}|${this.loginType}|${this.returnUrl}`);
  }
}
</script>
