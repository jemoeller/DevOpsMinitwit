# DevOpsMinitwit

Minitwit.py

---------------------IMPORTS--------------------------
import re  
import time  
import sqlite3  
from hashlib import md5  
from datetime import datetime  
from contextlib import closing  
from flask import Flask, request, session, url_for, redirect, \  
     render_template, abort, g, flash  
from werkzeug.security import check_password_hash, generate_password_hash  

---------------------CONFIG---------------------------

Connfig:
DATABASE = '/tmp/minitwit.db'
PER_PAGE = 30
DEBUG = True
SECRET_KEY = 'development key'

---------------------METHODS--------------------------

Methods:
def connect_db():  
    """Returns a new connection to the database."""

------------------------------------------------------

def init_db():  
    """Creates the database tables."""

------------------------------------------------------

def query_db(query, args=(), one=False):  
    """Queries the database and returns a list of dictionaries."""

------------------------------------------------------

def get_user_id(username):  
    """Convenience method to look up the id for a username."""

------------------------------------------------------

def format_datetime(timestamp):

------------------------------------------------------

def gravatar_url(email, size=80):

------------------------------------------------------

@app.before_request  
def before_request():  
    """Make sure we are connected to the database each request and look

------------------------------------------------------

@app.after_request  
def after_request(response):  
    """Closes the database again at the end of the request."""

------------------------------------------------------

@app.route('/')  
def timeline():  
    """Shows a users timeline or if no user is logged in it will
    redirect to the public timeline.  This timeline shows the user's
    messages as well as all the messages of followed users.
    """

------------------------------------------------------

@app.route('/public')  
def public_timeline():  
    """Displays the latest messages of all users."""

------------------------------------------------------

@app.route('/<username>')  
def user_timeline(username):  
    """Display's a users tweets."""

------------------------------------------------------

@app.route('/<username>/follow')  
def follow_user(username):  
    """Adds the current user as follower of the given user."""

------------------------------------------------------

@app.route('/<username>/unfollow')  
def unfollow_user(username):  
    """Removes the current user as follower of the given user."""

------------------------------------------------------

@app.route('/add_message', methods=['POST'])  
def add_message():  
    """Registers a new message for the user."""

------------------------------------------------------

@app.route('/login', methods=['GET', 'POST'])  
def login():  
    """Logs the user in."""

------------------------------------------------------

@app.route('/register', methods=['GET', 'POST'])  
def register():  
    """Registers the user."""

------------------------------------------------------

@app.route('/logout')  
def logout():  
    """Logs the user out"""


-------------------END OF FILE------------------------
# add some filters to jinja and set the secret key and debug mode  
# from the configuration.  
app.jinja_env.filters['datetimeformat'] = format_datetime  
app.jinja_env.filters['gravatar'] = gravatar_url  
app.secret_key = SECRET_KEY  
app.debug = DEBUG  


if __name__ == '__main__':  
    app.run(host="0.0.0.0")  
------------------------------------------------------
