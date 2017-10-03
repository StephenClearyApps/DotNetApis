const { CheckerPlugin } = require("awesome-typescript-loader");
const HtmlWebpackPlugin = require("html-webpack-plugin");

module.exports = env => {
    console.log(env.prod ? "Production build." : "Debug build.");
    return {
        entry: "./src/index.tsx",
        output: {
            filename: env.prod ? "static/[name].[chunkhash].js" : "static/bundle.js",
            path: __dirname + "/../docs"
        },

        resolve: {
            // Add '.ts' and '.tsx' as resolvable extensions.
            extensions: [".ts", ".tsx", ".js"],
            alias: {
                'ably': 'ably/browser/static/ably-commonjs.js'
            }
        },

        module: {
            rules: [
                // All files with a '.ts' or '.tsx' extension will be handled by 'awesome-typescript-loader'.
                {
                    test: /\.tsx?$/,
                    loader: "awesome-typescript-loader"
                },
                {
                    // All output '.js' files will have any sourcemaps re-processed by 'source-map-loader'.
                    enforce: 'pre',
                    test: /\.js$/,
                    loader: "source-map-loader"
                },
                {
                    test: /\.css$/,
                    use: [
                        { loader: "style-loader" },
                        { loader: "css-loader" }
                    ]
                }
            ]
        },

        plugins: [
            new CheckerPlugin(),
            new HtmlWebpackPlugin({
                template: "src/index.ejs",
                inject: false,
                minify: env.prod ? { collapseWhitespace: true } : false
            })
        ]
    };
}
