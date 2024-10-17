## Advanced Features

This page lists some of the more advanced or less-used features of the blog software.

### Shortcodes
Shortcodes are markdown content that can be shown inside blog posts (like templates that can be referenced).
The idea is to reuse certain shortcodes across various blog posts.
If you update the shortcode, it will be updated across all those blog posts as well.

For example if you have a running promotion you can add a shortcode and link it in various blog posts. Updating the shortcode (for example that it is almost sold out) will update all blog posts that reference this shortcode.

#### Creating a shortcode

To create a shortcode, click on "Shortcodes" in the Admin tab of the navigation bar. You can create a shortcode by adding a name in the top row and the markdown content in the editor. Clicking on an already existing shortcode will allow you to either edit the shortcode or delete it.

Currently, deleting a shortcode will leave the shortcode name inside the blogpost. Therefore only delete shortcodes if you are sure that they are not used anymore.

#### Using a shortcode
There are two ways:
 1. If you know the shortcode name, just type in `[[SHORTCODENAME]]` where `SHORTCODENAME` is the name you gave the shortcode.
 2. Click on the more button in the editor and select "Shortcodes". This will open a dialog where you can select the shortcode you want to insert and puts it into the clipboard.

### Limitations
Shortcodes
 * are not recursive. This means that you cannot use a shortcode inside a shortcode.
 * are not part of the table of contents even though they might have headers.
 * are not part of the reading time calculation.
 * are only available in the content section of a blog post and not the description.
 * are currently only copied to the clipboard and not inserted directly into the editor at the cursor position.