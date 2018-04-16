<template>
    <div id="app" class="container-fluid">
        <div class="row">
            <div class="col-sm-3">
                <nav-menu params="route: route"></nav-menu>
            </div>
            <div class="col-sm-9">
                <router-view></router-view>
            </div>
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
