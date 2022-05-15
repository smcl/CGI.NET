# CGI.NET

Toy implementation of CGI in C#. Configured with a couple of examples in python (so make sure `python` is in your `PATH`), `cgi-bin/hello`:

    $ curl https://localhost:5001/cgi-bin/hello --insecure
    hello from python

or `cgi-bin/hey`:

    $ curl https://localhost:5001/cgi-bin/hey --insecure -d "sean
    "
    hey sean from python

or `cgi-bin/ciao/{name}`:

    $ curl https://localhost:5001/cgi-bin/ciao/sean --insecure
    ciao, sean