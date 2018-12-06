<template>
  <div>
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
      <a
        class="navbar-brand"
        href="#"
      ><i class="material-icons logo-embed"></i> <span class="align-top">{{appName}}</span></a>
      <div id="navbarSupportedContent" class="ml-auto">
        <div class="form-inline my-2 my-lg-0">
          <!--i class="material-icons">account_circle</i-->
          <img v-bind:src="profileImage" class="avatar"/>
          <a class="btn btn-primary" href="javascript:void(0)" role="button"
            v-on:click="logout" v-if="showSignout">
            <span class="align-middle">Logout</span></a>
        </div>
      </div>
    </nav>
  </div>
</template>

<style lang="stylus" scoped>
.navbar-brand img
    width: 32px;
    height: 32px;
.navbar-brand i
  background-color: white;
  width: 24px;
  height: 24px;
  -webkit-mask-size: cover;
  mask-size: cover;
.logo-embed
  mask: embedurl("../Images/apogee-logo.svg", "utf8") no-repeat;
  -webkit-mask: embedurl("../Images/apogee-logo.svg", "utf8") no-repeat;
.avatar
  border-radius: 20px;
  height: 40px;
</style>

<script lang="ts">
import { Vue, Component, Prop, Provide } from "vue-property-decorator";
import { Api } from "./api";
import { RouteNames } from "../routes";
import { EventNames } from "../events";

@Component
export default class NavBar extends Vue {
  @Provide() appName: string = "Apogee-Dev";
  @Provide() showSignout: boolean = false;
  @Provide() profileImage: string = "";
  readonly api: Api = new Api();

  constructor() {
    super();
    let self = this;
    self.$eventBus.$on(EventNames.LoginSuccess, function() {
      self.$nextTick(function() {
        self.toggleSignout(true);
        console.log(EventNames.LoginSuccess);
      });
    });
    self.$eventBus.$on(EventNames.LoginFailure, function() {
      self.$nextTick(function() {
        self.toggleSignout(false);
        console.log(EventNames.LoginFailure);
      })
    });
  }

  logout(event: Event) {
    event.stopPropagation();
    event.preventDefault();
    let self = this;
    self.api.logout().finally(function() {
      self.$nextTick(function() {
        self.toggleSignout(false);
      });
      self.$router.push({ name: RouteNames.Login });
    });
  }

  toggleSignout(show: boolean) {
    this.showSignout = show;
    this.profileImage = show ? this.api.getUserProfile().imageUrl : "";
    console.log("root logged in");
  }
}
</script>

