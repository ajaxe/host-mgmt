<template>
  <div class="card-text">
    <h3 class="text-muted">Manage API Keys</h3>
    <p>Manage and create credentials for using services at:
      <ul class="list-group">
        <li class="list-group-item">docker-registry.apogee-dev.com</li>
        <li class="list-group-item">faas.apogee-dev.com/ui/</li>
      </ul>
    </p>
  </div>
</template>
<script lang="ts">
import { Vue, Component, Prop, Provide } from "vue-property-decorator";
import { Route, RawLocation } from "vue-router";
import { Api } from "./api";
import { RouteNames } from "../routes";
import { EventNames } from "../events";

@Component({
  beforeRouteEnter: (
    to: Route,
    from: Route,
    next: (to?: RawLocation | false | ((vm: Vue) => any) | void) => void
  ) => {
    let api: Api = new Api();
    api
      .checkUserSession()
      .then(function(isValid) {
        if(isValid) {
          next();
        }
        else {
          next({ name: RouteNames.Login });
        }
      })
      .catch(function() {
        next({ name: RouteNames.Login });
      });
  },
  mounted: function() {
    this.$eventBus.$emit(EventNames.LoginSuccess);
  }
})
export default class Home extends Vue {}
</script>

