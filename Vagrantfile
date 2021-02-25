Vagrant.configure('2') do |config|

config.vm.define "droplet" do |config|
	config.vm.provider :digital_ocean do |provider, override|
override.ssh.private_key_path = '~/.ssh/id_rsa'
        override.vm.box = 'digital_ocean'
        override.vm.box_url = "https://github.com/devopsgroup-io/vagrant-digitalocean/raw/master/box/digital_ocean.box"
	config.vm.network :forwarded_port, guest: 8080, host: 8080
	config.vm.network :forwarded_port, guest: 5432, host: 5432
        override.nfs.functional = false
        override.vm.allowed_synced_folder_types = :rsync
        provider.token = '{YOUR TOKEN}'
        provider.image = 'ubuntu-18-04-x64'
        provider.region = 'nyc1'
        provider.size = 's-1vcpu-1gb'
        provider.backups_enabled = false
        provider.private_networking = false
        provider.ipv6 = false
        provider.monitoring = false
	config.vm.provision "shell", privileged: false, inline: <<-SHELL
	echo "Installing git"
	sudo apt-get install git
	echo "Cloning Minitwit"
	git clone https://github.com/SanderBuK/DevOpsMinitwit
	echo "Installing dotnet 3.1"
	wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
	sudo dpkg -i packages-microsoft-prod.deb
	sudo apt-get update; \
  	sudo apt-get install -y apt-transport-https && \
  	sudo apt-get update && \
  	sudo apt-get install -y dotnet-sdk-5.0
	dotnet run DevOpsMinitwit/MiniTwitAPI/MiniTwit.API
	SHELL
	end
  end
end
