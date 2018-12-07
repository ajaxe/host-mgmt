<template>
  <div class="card-text">
    <h3 class="text-muted">Manage API Keys</h3>
    <p>Manage and create credentials for using services.</p>
    <table class="table">
      <thead>
        <tr>
          <th>Services</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>docker-registry.apogee-dev.com</td>
        </tr>
        <tr>
          <td>faas.apogee-dev.com/ui/</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
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
export default class Home extends Vue {}
</script>

