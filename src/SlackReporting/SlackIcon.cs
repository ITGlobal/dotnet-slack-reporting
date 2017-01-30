using System;
using System.Collections.Generic;

namespace ITGlobal.SlackReporting
{
    /// <summary>
    ///     Slack icon
    /// </summary>
    public abstract class SlackIcon
    {
        /// <summary>
        ///     Icon from an URL
        /// </summary>
        public static SlackIcon Url(string url) => new UrlSlackIcon(url);

        /// <summary>
        ///     Icon from an emoji
        /// </summary>
        public static SlackIcon Emoji(string id) => new EmojiSlackIcon(id);

        internal abstract void SetIcon(IDictionary<string, string> args);

        private sealed class UrlSlackIcon: SlackIcon
        {
            private readonly string _url;

            public UrlSlackIcon(string url)
            {
                if(string.IsNullOrEmpty(url))
                    throw new ArgumentNullException(nameof(url));
                if(!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                    throw new ArgumentException(nameof(url));

                _url = url;
            }

            internal override void SetIcon(IDictionary<string, string> args)
            {
                args["icon_url"] = _url;
            }
        }

        private sealed class EmojiSlackIcon : SlackIcon
        {
            private readonly string _id;

            public EmojiSlackIcon(string id)
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                _id = id;
            }

            internal override void SetIcon(IDictionary<string, string> args)
            {
                args["icon_emoji"] = _id;
            }
        }
    }
}
