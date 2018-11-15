<template>
    <div class="container">
        <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
            <a class="navbar-brand" href="#">Welcome {{userName}}</a>
            <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>

            <div class="collapse navbar-collapse" id="navbarSupportedContent">
                <ul class="navbar-nav mr-auto">
                    <li v-for="route in routes" class="nav-item">
                        <router-link :to="route.path"> <span :class="route.style"></span> {{ route.display }}</router-link>
                    </li>
                </ul>
                <form class="form-inline">
                    <button type="button" @click="authenticate()" v-if="!isAuthenticated" class="btn">
                        Login
                    </button>
                    <button type="button" @click="logout()" v-if="isAuthenticated" class="btn">
                        Logout
                    </button>
                </form>
            </div>
        </nav>
    </div>
    <!--<nav class="navbar navbar-dark bg-dark navbar-expand-md">
        <a class="navbar-brand" href="/">Welcome {{userName}}</a>
        <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent"
                aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>

        <div class="navbar-collapse collapse show"  id="navbarSupportedContent" v-show="!collapsed">
            <ul class="nav navbar-nav">
                <li v-for="route in routes" class="nav-item">
                    <router-link :to="route.path"> <span :class="route.style"></span> {{ route.display }}</router-link>
                </li>
            </ul>
        </div>


        <button type="button" @click="authenticate()" v-if="!isAuthenticated" class="btn">
            Login
        </button>
        <button type="button" @click="logout()" v-if="isAuthenticated" class="btn">
            Logout
        </button>



    </nav>-->

</template>

<script>
import { routes } from "../routes";
import { UserManager } from "oidc-client";
import config from "../config";
export default {
  data() {
    return {
      routes,
      collapsed: true,
      userManager: new UserManager(config),
      isAuthenticated: false,
      userName: ""
    };
  },
  created() {
    if (this.isAuthResponse()) {
      this.completeAuthentication();
    }
  },
  methods: {
    toggleCollapsed: function(event) {
      this.collapsed = !this.collapsed;
    },
    getUserManager() {
      return this.userManager;
    },
    isAuthResponse() {
      return this.$route.fullPath.indexOf("id_token") > -1;
    },
    authenticate() {
      console.log("login");
      this.isAuthenticated = false;
      this.userName = "";
      this.$store.commit("updatePreviousLocation", this.$router.currentRoute.fullPath);
      return this.userManager.signinRedirect();
    },
    logout() {
      console.log("logout");
      this.userManager
        .getUser()
        .then(user => {
          this.userManager
            .signoutRedirect({})
            .then(d => {
              console.log("success signout");
              console.log(d);
            })
            .catch(e => {
              console.log(e);
            });
        })
    },
    completeAuthentication() {
      console.log("complete authentication called");
      return this.userManager
        .signinRedirectCallback()
        .then(data => {
          console.log(data);
          this.isAuthenticated = !data.expired;
          this.userName = data.profile.name;
          this.$store.commit("user", {
            user: data
          });
          this.fixHistory()
        })
        .catch(e => {
          console.log("error callback");
          this.isAuthenticated = false;
          this.userName = "";
          console.log(e);
        });
    },
    fixHistory() {
      // avoids that the user can navigate back into the login screen
      window.history.replaceState(
            {},
            window.document.title,
            window.location.origin + window.location.pathname
          );
      let previousLocation = this.$store.state.auth.previousLocation;
      if (previousLocation && previousLocation.startsWith("/")) {
        this.$router.push(previousLocation);
      } else {
        this.$router.push("/");
      }
    }
  }
};
</script>

<style>
.slide-enter-active,
.slide-leave-active {
  transition: max-height 0.35s;
}
.slide-enter,
.slide-leave-to {
  max-height: 0px;
}

.slide-enter-to,
.slide-leave {
  max-height: 20em;
}
</style>
