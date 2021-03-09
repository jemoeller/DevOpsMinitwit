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
			provider.ssh_key_name = ENV["DIGITAL_OCEAN_KEYNAME"]#create or read public key on DigitalOcean
			provider.token = ENV["DIGITAL_OCEAN_TOKEN"]#Use token to create droplet on DigitalOcean
			provider.image = 'docker-18-04'#Choose droplet image to create
			provider.region = 'fra1'#select which region droplet is located in
			provider.size = 's-1vcpu-1gb'#select cpu and so on for droplet
			provider.privatenetworking = true
		end

		#'env:' allows us to use local environment variables in the server provision. They will NOT be accessible outside of the provision.
		server.vm.provision "shell",
		env: {
			"DOCKER_PW"=>ENV['DOCKER_PW'],
			"DOCKER_ID"=>ENV['DOCKER_ID'],
			"GITHUB_TOKEN"=>ENV['GITHUB_TOKEN'],
			"CONNECTION_STRING"=>ENV['CONNECTION_STRING']}, 
		inline: <<-SHELL
		echo pulling git repository
		git clone --single-branch --branch feature/36/setupScript https://$GITHUB_TOKEN:x-oauth-basic@github.com/SanderBuK/DevOpsMinitwit.git
		echo login docker
		echo "$DOCKER_PW" > ~/my_password.txt
		cat ~/my_password.txt |docker login -u "${DOCKER_ID}" --password-stdin
		rm ~/my_password.txt
		docker pull jemol/minitwit_blazor:latest
		docker pull jemol/minitwit_api:latest
		docker run -d -p 5001:80 jemol/minitwit_api:latest
		docker run -d -p 8001:80 jemol/minitwit_blazor:latest
		docker logout
		SHELL
	end
end