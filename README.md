# Simple photoblogging web-app

That's what this repository contains. Imagine a very stripped-down version of Instagram. Why am I writing something that I can get as a free service? First of all, I don't want to give my private photos to some huge company that makes money solely on gathering and analyzing of user data. Secondly, I just like to try out some new technology every now and then. And a small project like this is a good opportunity to do just that. This time, it's .NET Core.

Here's a few notes about the app itself:
- It's not designed for high-traffic or a large number of users. In my case, it's a single-user app. Hence, all content is visible to all users.
- All data is stored in plain files, including password hashes. I host the app on my own server, so I have full control over who can access those files.
- I don't really mind seeing exception messages on the client side. Quite the opposite, actually.
