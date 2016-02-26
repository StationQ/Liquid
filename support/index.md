---
layout: main
title: How to get help?
---

# Stuck?
If you need help there are a lot of ways you can connect with the team and seek out assistance.

## StackOverflow
StackOverflow is one of the most used sites for asking general purpose programming questions, and no wonder there is already a [number of existing questions](http://stackoverflow.com/questions/tagged/coreclr) tagged with **coreclr** as well as [questions about Roslyn](http://stackoverflow.com/questions/tagged/roslyn).

## Gitter channels
[Gitter](https://gitter.im/) is IRC for GitHub projects. There are always people there ready to help with various questions around .NET Core, CoreFX and Roslyn.

*  [dotnet/coreclr](https://gitter.im/dotnet/coreclr)
*  [dotnet/corefx](https://gitter.im/dotnet/corefx)
*  [dotnet/roslyn](https://gitter.im/dotnet/roslyn)
*  [dotnet/cli](https://gitter.im/dotnet/cli)

## Blogs
Call us old school, but we do love to write detailed content for people to consume, and our blogs are the main outlet where you can find this content.  

* [DotNet Blog](http://blogs.msdn.com/b/dotnet/)
* [Channel9 .NET Blogs](http://channel9.msdn.com/Blogs/dotnet)
* [.NET Web Development and Tools Blog](http://blogs.msdn.com/b/webdev/)

## Twitter
[@dotnet on Twitter](https://twitter.com/DotNet) is the main Twitter account you can follow to keep in touch with the team and get updates.

Of course, do not miss out on these other teams that are closely related to .NET Core.

* [@VisualFSharp](https://slack-redir.net/link?url=https%3A%2F%2Ftwitter.com%2FVisualFSharp&v=3)
* [@aspnet](https://twitter.com/aspnet)
* [@efmagicunicorns](https://twitter.com/efmagicunicorns)

## Facebook
If you're more of a Facebook fan, you can also visit our Facebook pages on [https://www.facebook.com/Dotnet](https://www.facebook.com/Dotnet) and [https://www.facebook.com/asp.net](https://www.facebook.com/asp.net).

## Public forums
.NET Foundation keeps an active public forum (BBS-like) that you can find on [.NET Foundation Forums](http://forums.dotnetfoundation.org/).

# Community meetings/meetups 
Here is a list of events for the set of .NET user group meetups from [meetup.com](http://meetup.com) and [Community Megaphone](http://communitymegaphone.com/).

We also maintain a much larger list of [.NET meetup groups on Twitter](https://twitter.com/DotNet/dotnet-user-groups). If you would like your group's events listed on either list, please contact us, on [Twitter](http://twitter.com/dotnet).

<div id="communityContainer">
    <div id="ugResults">
        <!-- ko if: !loaded -->
            <div id="ugLoader">Loading...</div>
        <!-- /ko -->
        <div id="meetup-table" data-bind="if: loaded">
            <table class="community-table">
                <tbody data-bind="foreach: events">
                    <tr>
                        <td data-bind="text: dateTime"></td>
                        <td><img data-bind="attr: { src: source().image }" class="ug-badge" /></td>
                        <td><a data-bind="attr: { href: url }, text: name"></a></td>
                        <td><a data-bind="attr: { href: orgGroup().url }, text: orgGroup().name"></a></td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</div>
<script src="//ajax.aspnetcdn.com/ajax/knockout/knockout-3.3.0.js"></script>
<script src="//cdnjs.cloudflare.com/ajax/libs/moment.js/2.10.3/moment.min.js"></script>
<script src="/js/meetup-ko.js"></script>

