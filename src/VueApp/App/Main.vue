<template>
  <div id="app">
    <nav-bar></nav-bar>
    <section class="container-fluid">
      <div class="row justify-content-center">
        <div class="col-12 col-md-6">
          <div class="card mt-5">
            <div class="card-body">
              <h2 class="card-title">API Key Management</h2>
              <router-view></router-view>
            </div>
            <div class="card-footer" v-if="showNavigation">
              <nav class="nav nav-pills nav-fill" v-on:click="onNavigationClick">
                <router-link to="/home" class="nav-item nav-link">
                  <i class="material-icons">vpn_key</i>
                  <div class="d-none d-sm-block">API Keys</div>
                </router-link>
                <router-link to="/home" class="nav-item nav-link">
                  <i class="material-icons">person</i>
                  <div class="d-none d-sm-block">Account</div></router-link>
              </nav>
            </div>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<style lang="stylus" scoped>
.card-body
  height: calc(100vh - 300px);
</style>


<script lang="ts">
import { Vue, Component, Prop, Provide } from "vue-property-decorator";
import VueRouter, { RouteConfig } from "vue-router";
import Login from "./Components/Login.vue";
import Home from "./Components/Home.vue";
import NavBar from "./Components/NavBar.vue";
import { RouteNames } from "./routes";
import { EventNames } from "./events";

Vue.prototype.$eventBus = new Vue({});

const routes: RouteConfig[] = [
  { path: "/login", component: Login, name: RouteNames.Login },
  { path: "/home", component: Home, name: RouteNames.Home },
  { path: "/", redirect: "/home" }
];

const AppRouter = new VueRouter({
  routes // short for `routes: routes`
});

Vue.prototype.$eventBus = new Vue({});

@Component({
  components: {
    NavBar
  },
  router: AppRouter
})
export default class Main extends Vue {
  @Provide() showNavigation: boolean = false;

  constructor() {
    super();
    let self = this;
    self.$eventBus.$on(EventNames.LoginSuccess, function() {
      self.toggleNavigation(true);
      console.log(EventNames.LoginSuccess);
    });
    self.$eventBus.$on(EventNames.LoginFailure, function() {
      self.toggleNavigation(false);
      console.log(EventNames.LoginFailure);
    });
  }

  onNavigationClick = function() {
    console.log('onNavigationClick');
    console.log(arguments);
  }

  toggleNavigation(show: boolean) {
    this.showNavigation = show;
    console.log("root logged in");
  }
}
</script>

