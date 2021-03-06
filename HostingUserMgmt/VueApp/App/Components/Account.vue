<template>
  <div class="card-text">
    <h3 class="text-muted">Account</h3>
    <div class="row">
      <div class="col">
        <div class="form-group row">
          <label
            for="staticEmail"
            class="col-sm-2 col-form-label"
          >Email</label>
          <div class="col-sm-10">
            <input
              type="text"
              readonly
              class="form-control-plaintext"
              id="staticEmail"
              :value="emailAddress"
            >
          </div>
        </div>
        <div class="form-group row">
          <label
            for="name"
            class="col-sm-2 col-form-label"
          >Name</label>
          <div class="col-sm-10">
            <input
              type="text"
              readonly
              class="form-control-plaintext"
              id="name"
              :value="name"
            >
          </div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col">
        <button type="button" class="btn btn-raised btn-danger" v-on:click="deleteUser">Delete Account</button>
        <p class="text-warning">Deleting account will remove associated API Keys and removing any access to the services.</p>
      </div>
    </div>
  </div>
</template>

<style lang="stylus" scoped>
.avatar
  border-radius: 100px;
  height: 200px;
</style>


<script lang="ts">
import { Vue, Component, Prop, Provide } from "vue-property-decorator";
import { Route, RawLocation } from "vue-router";
import { Api } from "./api";
import { RouteNames } from "../routes";
import { EventNames } from "../events";
import { AuthHelpers } from "./authHelpers";

@Component({
  beforeRouteEnter: (
    to: Route,
    from: Route,
    next: (to?: RawLocation | false | ((vm: Vue) => any) | void) => void
  ) => {
    AuthHelpers.checkRouteAuthorization(to, from, next);
  },
  mounted: function() {
    this.$eventBus.$emit(EventNames.LoginSuccess);
  }
})
export default class Account extends Vue {
  readonly api: Api = new Api();
  @Provide() name: string = "";
  @Provide() emailAddress: string = "";
  @Provide() profileImage: string = "";

  constructor() {
    super();
    var self = this;
    self.$nextTick(function(){ self.setProfile(); })
  }

  setProfile(): void {
    let profile = this.api.getUserProfile();
    this.name = profile.name;
    this.emailAddress = profile.emailAddress;
    this.profileImage = this.setProfileImageHeight(profile.imageUrl, 40);
  }
  setProfileImageHeight(imageUrl: string, size: number): string {
    if(!imageUrl) {
      return;
    }
    let url = imageUrl.split("?")[0];
    let sz = size < 40 ? 40 : size;
    return `${url}?sz=${sz}`;
  }
  deleteUser(event: Event): void {
    let self = this;
    self.api.deleteUser()
    .then(function(){
      return self.api.logout();
    })
    .finally(function(){
      document.location.reload(true);
    });
  }
}
</script>

