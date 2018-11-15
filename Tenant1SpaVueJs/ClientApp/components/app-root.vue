<template>
    <div id="app" class="container">
        <div class="row">
          
                <nav-menu params="route: route"></nav-menu>
          
          
                <router-view></router-view>
          
        </div>
    </div>

</template>

<script>
    import Vue from 'vue'
    import HomePage from './home-page'
    import NavMenu from './nav-menu'

    Vue.component('home-page', HomePage);
    Vue.component('nav-menu', NavMenu);

    export default {
    data() {
    return {
    }
    }
    }
</script>

<style>
</style>
