# SoundCloud-Unity
> A wrapper of the SoundCloud® API for use in Unity projects.

**STATUS: NOT READY. Don't try to use this yet. It doesn't work.**

* **Access SoundCloud API Data** - Including users, tracks, playlists, groups, and comments.
* **Play Tracks from SoundCloud** - Generate an AudioClip from any SoundCloud track URL or ID that you can pass along to an AudioSource and playback in your game.
* **Basic Authentication Support** - Allow users to sign in to their own SoundCloud account to allow access to functions such as liking tracks and following other users.
* **Windows, OSX, Linux** - Basic data retrieval should work anywhere. Full functionality on iOS/Android is possible but not currently a consideration. Unsure about the potential of full functionality on other platforms (PS4, Xbox One, etc.) - at a minimum it would require significant changes to the way audio transcoding is handled.

## Using SoundCloud-Unity

*TODO*

* Install.
* Register App. (http://soundcloud.com/you/apps)
* Fill in Client ID.
* Get latest ffmpeg converters.

## SoundCloud API Info
### Use Requirements
Use of the SoundCloud API in your game requires you to meet SoundCloud's [Terms of Use][] and [Attribution Guidelines][]. These requirements include but are not limited to:

1. Credit the uploader as the creator of the sound.
1. Credit SoundCloud as the source by including one of the logos found [here][Attribution Guidelines].
1. If the sound is public, link to the SoundCloud URL containing the work.
1. If the sound is private, link to the profile of the creator.

An attempt has been made to document in this library's code things that are done to comply with the API [Terms of Use][]. Please do not attempt to remove these features.

### API Documentation
Full documentation of the SoundCloud API can be found at the [SoundCloud Developers][] site.

### API Rate Limit
As of July 1, 2015, SoundCloud limits applications to 15,000 streams per day. More info [here][Rate Limit Announcement].

If you encounter an issue with this, you can try applying for an increased rate limit [here][High Volume App] - no clue about the success rate of those who apply. Alternatively, you could try modifying this library to allow users to enter their own application client ID which would allow individual users to stream 15,000 sounds per day rather than all users using the same client ID and being limited to 15,000 streams per day among all users.

## Included Libraries
* [Full Serializer](https://github.com/jacobdufault/fullserializer)

## Credits
* **Chase Pettit** - [github](https://github.com/Chaser324), [twitter](http://twitter.com/chasepettit)
* **Thanks** - All of the [Drift Stage][] backers and supporters.

## License
All code in this repository ([soundcloud-unity](https://github.com/Chaser324/soundcloud-unity)) is made freely available under the MIT license.

[Terms of Use]: https://developers.soundcloud.com/docs/api/terms-of-use
[Attribution Guidelines]: https://developers.soundcloud.com/docs/api/buttons-logos
[SoundCloud Developers]: https://developers.soundcloud.com/
[Rate Limit Announcement]: https://developers.soundcloud.com/blog/limits
[High Volume App]: https://docs.google.com/forms/d/16Vs0Ikv2HqH8UJPGmX5kF2Wy4Rx_qMpgDrL39ACJKyE/viewform
[Drift Stage]: http://www.driftstagegame.com/