<template>
    <div class="alert alert-success alert-dismissible fade show" role="alert">
        <p>
            New Api key has been created. Please copy the API key, it will not shown again.<br/>
            <strong>Key Name:</strong><span v-text="newApiKey.keyName"></span><br/>
            <strong>API Key:</strong><span v-text="newApiKey.keySecret"></span><br/>
        </p>
        <button type="button" class="close" data-dismiss="alert" aria-label="Close"
            v-on:click="$emit('close')">
        <span aria-hidden="true">&times;</span>
        </button>
    </div>
</template>

<script lang="ts">
import { Vue, Component, Prop, Provide } from "vue-property-decorator";
import { NewApiKey } from "./Types/newApiKey";

@Component
export default class NewApiKeyAlert extends Vue {
  @Prop() newApiKey: NewApiKey;
}
</script>
