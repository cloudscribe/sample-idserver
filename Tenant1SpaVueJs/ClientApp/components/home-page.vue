<template>
    <div class="container">
        <div class="row">
            <div class="col">
                <button @click="CallApi()">Call Api</button>
                <div>Identity seen through the eyes of the API</div>
                <pre>{{identityData}}</pre>
            </div>

            <div class="col">

                <div>Claims: Identity seen through the eyes of the spa application:</div>
                <pre> {{claimsData}}</pre>
            </div>
        </div>
    </div>
</template>

<script>
import config from "../config";
import Vue from "vue";
export default {
  computed: {
    claimsData() {
      return this.$store.state.auth.user.profile;
    }
  },
  data() {
    return {
      identityData: {}
      //claimsData: {}
    };
  },
  methods: {
    CallApi() {
      this.axios.get(config.apiUrl).then(d => {
        this.identityData = d.data;
      });
    }
  },
  created() {}
};
</script>
<style>

</style>
