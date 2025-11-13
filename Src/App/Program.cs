// -----------------------------------------------------------------------------
// File Responsibility: Application entry point that constructs the bootstrapper
// and starts the asynchronous game loop.
// Key Members: Program.Main invoking Bootstrapper.RunAsync.
// -----------------------------------------------------------------------------
using Linebreak.App;

Bootstrapper bootstrapper = new Bootstrapper();
await bootstrapper.RunAsync(args);

