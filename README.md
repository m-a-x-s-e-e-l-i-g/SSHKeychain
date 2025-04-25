# SSH Keychain Command Palette Extension

A Windows Command Palette extension that allows you to quickly access your SSH connections directly from the Command Palette.  
As an alternative to my GUI-based App: [VerySSH](https://github.com/m-a-x-s-e-e-l-i-g/very-ssh)

## Features

- Automatically discovers SSH hosts from your SSH config file (`~/.ssh/config`)
- Displays all your SSH connections in the Windows Command Palette
- One-click connection to any SSH host

## Requirements

- Windows 10/11 with Windows Command Palette support. [Install PowerToys](https://aka.ms/installpowertoys)
- SSH client configured with a valid `~/.ssh/config` file

## Usage

1. Open Windows Command Palette (Win+Alt+Space)
2. Type "SSH" to filter for SSH connections
3. Select the host you want to connect to
4. Terminal will automatically open with an SSH session to the selected host

## Configuration

The extension automatically reads your SSH configuration from `~/.ssh/config`. Make sure this file exists and is properly formatted. Example:

```
Host github
    HostName github.com
    User git
    IdentityFile ~/.ssh/id_github

Host myserver
    HostName example.com
    User admin
    Port 2222
    IdentityFile ~/.ssh/id_rsa
```

## Building from Source

1. Clone the repository
2. Open the solution in Visual Studio 2022 or later
3. Build the solution
4. Deploy the package to your local machine for testing
5. Reload Extension in Windows Command Palette by typing "Reload Extension" in the Command Palette

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
