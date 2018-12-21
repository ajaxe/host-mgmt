<template>
  <tr v-bind:data-id="keyId">
    <td>{{keyId}}</td>
    <td>{{keyName}}</td>
    <td>{{createdAtUtc | formatDate}}</td>
    <td>
      <a href="javascript:void(0)" data-toggle="tooltip" data-placement="top" title="Delete API Key" v-on:click="deleteKey">
        <i class="material-icons">close</i>
      </a>
    </td>
  </tr>
</template>

<script lang="ts">
import { Vue, Component, Prop, Provide } from "vue-property-decorator";
import { ApiCredential } from "./Types/apiCredential";
import { Api } from "./api";

@Component
export default class ApiKeyRow extends Vue {
  @Prop() keyId: number;
  @Prop() keyName: string;
  @Prop() createdAtUtc: Date;

  readonly api: Api = new Api();

  deleteKey(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    let self = this;
    self.api.deleteApiKeyById(self.keyId)
    .then(function(keyData) {
      self.$emit("delete", { keyId: self.keyId });
    });
  }
};
</script>

