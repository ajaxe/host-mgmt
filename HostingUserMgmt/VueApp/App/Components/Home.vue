<template>
  <div class="card-text" id="home">
    <h3 class="text-muted">Manage API Keys</h3>
    <p>Manage and create credentials for using services.</p>
    <div class="row">
      <div class="col-12 col-sm-5 order-1 order-sm-2">
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
      <div class="col-12 col-sm-7 order-2 order-sm-1">
        <button type="button" class="btn btn-raised btn-primary" v-on:click="addApiKey">
          <span class=material-icons>add</span>
          <span class="text">Add API Key</span></button>
        <new-api-key-alert v-on:close="clearNewApiKey"
          v-if="hasNewApiKey()"
          v-bind:new-api-key="newApiKey"></new-api-key-alert>
        <div class="table-responsive">
          <table class="table">
            <thead>
              <tr>
                <th>#</th>
                <th>API Key Name</th>
                <th>Created On</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <tr is="api-key-row" v-for="k in keys"
                v-bind:key="k.key" v-bind:key-name="k.apiKeyName"
                v-bind:key-id="k.apiKeyId" v-bind:created-at-utc="k.createdAtUtc"
                v-on:delete="removeKey">
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { Vue, Component, Prop, Provide } from "vue-property-decorator";
import { Route, RawLocation } from "vue-router";
import $ from 'jquery';
import { Api } from "./api";
import { RouteNames } from "../routes";
import { EventNames } from "../events";
import { AuthHelpers } from "./authHelpers";
import { ApiCredential } from "./Types/apiCredential";
import { NewApiKey } from "./Types/newApiKey";
import ApiKeyRow from  "./ApiKeyRow.vue";
import NewApiKeyAlert from "./NewApiKeyAlert.vue";

@Component({
  components: {
    ApiKeyRow,
    NewApiKeyAlert
  },
  beforeRouteEnter: (
    to: Route,
    from: Route,
    next: (to?: RawLocation | false | ((vm: Vue) => any) | void) => void
  ) => {
    AuthHelpers.checkRouteAuthorization(to, from, next);
  },
  mounted: function() {
    this.$eventBus.$emit(EventNames.LoginSuccess);
    this.$eventBus.$emit(EventNames.HomeMounted);
  }
})
export default class Home extends Vue {
  @Provide() keys: ApiCredential[] = [];
  readonly api: Api = new Api();
  private newApiKey: NewApiKey = null;

  constructor() {
    super();
    let self = this;
    self.$eventBus.$once(EventNames.HomeMounted, function() {
      self.renderApiKeys();
      console.log(EventNames.HomeMounted);
    });
  }

  renderApiKeys() : void {
    let self = this;
    this.api.getApiKeys()
    .then(function(keys) {
      self.keys = keys;
      self.$nextTick(function() {
        $(document.querySelectorAll("#home [data-toggle=tooltip]")).tooltip();
      });
    });
  }

  addApiKey(event: Event): void {
    let self = this;
    self.api.createApiKey()
    .then(function(key){
      self.newApiKey = key;
      self.renderApiKeys();
    });
  }

  clearNewApiKey(): void {
    this.newApiKey = null;
  }

  hasNewApiKey(): boolean {
    return !!this.newApiKey;
  }

  newApiKeyName(): string {
    return this.newApiKey ? this.newApiKey.keyName : '';
  }

  newApiKeySecret(): string {
    return this.newApiKey ? this.newApiKey.keySecret : '';
  }

  removeKey(data): void {
    if(!data.keyId) {
      return;
    }
    let keyid = <number>data.keyId;
    let keyToRemove = this.keys.findIndex((cred, i, arr): boolean => {
      return cred.apiKeyId === keyid
    });
    this.keys.splice(keyToRemove, 1);
    this.$nextTick(function() {
      this.removeTooltip(keyid);
    });

  }

  removeTooltip(id: number): void {
    $(`#home tr[data-id=${id}] [data-toggle=tooltip]`).tooltip('dispose');
  }
}
</script>

