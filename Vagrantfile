# -*- mode: ruby -*-
# vi: set ft=ruby :

Vagrant.configure("2") do |config|
	config.vm.box = 'digital_ocean'#specify which VM provider you want
	config.vm.box_url = "https://github.com/devopsgroup-io/vagrant-digitalocean/raw/master/box/digital_ocean.box"#specify where to get the box
	config.ssh.private_key_path = '~/.ssh/id_rsa'
  
	config.vm.synced_folder ".", "/vagrant", disabled: true#sync folder between host and guest. Does not work for me

	#Create a droplet with the define name. Needs a token from digitalocean.
	config.vm.define "NavngivDropletHer", primary: true do |server|
	  server.vm.provider :digital_ocean do |provider|
		provider.ssh_key_name = "jm laptop"#create or read public key on DigitalOcean
		provider.token = ENV["DIGITAL_OCEAN_TOKEN"]#Use token to create droplet on DigitalOcean
		provider.image = 'docker-20-04'#Choose droplet image to create
		provider.region = 'fra1'#select which region droplet is located in
		provider.size = 's-1vcpu-2gb'#select cpu and so on for droplet
		provider.privatenetworking = true
	  end

	  #ENV allows us to use local environment variables in the server provision. They will NOT be accessible outside of the provision.
	  server.vm.provision "shell", 
	  env: 
	  {"GITHUB_TOKEN"=>ENV['GITHUB_TOKEN']}, 
	  inline: <<-SHELL
	  git clone -- single-branch feature/36/setupScript https://$GITHUB_TOKEN:x-oauth-basic@github.com/SanderBuK/DevOpsMinitwit.git
	  ./setup.sh
	  ./start.sh
		SHELL
	end
  end
	#wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
	#sudo dpkg -i packages-microsoft-prod.deb
	#sudo apt-get update; \
  	#sudo apt-get install -y apt-transport-https && \
  	#sudo apt-get update && \
  	#sudo apt-get install -y dotnet-sdk-5.0	
	#echo "Setting up API"
	#dotnet run --project DevOpsMinitwit/MiniTwitAPI/MiniTwit.API/ --urls=http://0.0.0.0:5001

