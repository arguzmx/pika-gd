const path = require("path");
const webpack = require('webpack');
const HtmlWebpackPlugin = require("html-webpack-plugin");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const MinifyPlugin = require('babel-minify-webpack-plugin');


module.exports = {
    entry: {
        'ETabular': "./wwwroot/js/componentes/EditorTabular/ETabular.js",
        'EMetadatos': "./wwwroot/js/componentes/EditorMetadatos/EMetadatos.js",
        'AppLayout': "./wwwroot/js/componentes/comunes/app_layour.js",
        'AppLayoutCSS': "./wwwroot/js/componentes/comunes/app_css.js"
    },
    output: {
        path: path.resolve(__dirname, "wwwroot/dist"),
        filename: "[name]-mod.js",
        publicPath: "dist/"
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                use: { loader: 'babel-loader', options: { presets: ['@babel/preset-env'] } }
            },
            {
                test: /\.css$/,
                use: [MiniCssExtractPlugin.loader, "css-loader"]
            }
        ]
    },
    plugins: [
        new webpack.ProvidePlugin({
            $: 'jquery',
            jQuery: 'jquery',
            'window.jQuery': 'jquery',
            Popper: ['popper.js', 'default']
        }),
        new CleanWebpackPlugin(),
        new HtmlWebpackPlugin(),
        new MiniCssExtractPlugin({
            filename: "css/[name].css"
        })
    ]
};