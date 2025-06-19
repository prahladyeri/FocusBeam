/*
init.sql - Build the schema

@author Prahlad Yeri <prahladyeri@yahoo.com>
@license MIT
 */
drop table if exists projects;
drop table if exists tasks;
drop table if exists timesheet;
drop table if exists mindmaps;
drop table if exists mcq;

create table projects (
	id integer primary key,
	title text not null unique,
	category int not null, -- enum
	tags text, -- "vim,php,python,python-mysql"
	start_date datetime not null,
	end_date datetime not null,
	notes text
);

create table tasks (
	id integer primary key,
	project_id int not null,
	parent_task_id int, -- nullable
	title text not null,
	priority int not null, -- enum
	status int not null, -- enum
	start_date datetime,
	end_date datetime,
	tags text,
	planned_hours int, -- no. of hours to be spend on this task each day.
	notes text,
	unique(project_id, title),
	foreign key (project_id) references projects(id),
	foreign key (parent_task_id) references tasks (id)
);

create table timesheet (
	id integer primary key,
	task_id int,
	start_time datetime,
	end_time datetime,
	duration int not null,-- Duration in minutes (e.g., 25 for a standard Pomodoro),
	status int not null, -- enum
	notes text,
	foreign key (task_id) references tasks (id)
);

create table mindmaps (
	id integer primary key,
	project_id int not null,
	type text check (type in ('Idea', 'Note', 'Question', 'Reference')) default 'Idea',
	parent_id int default 0, -- parent item's id or null in case of top level
	pos int default 0, -- position for custom ordering
	title text,
	tags text,
	color text,
	notes text,
	unique(project_id, title),
	foreign key (project_id) references projects (id),
	foreign key (parent_id) references mindmaps (id)
);

create table mcq (
	id integer primary key,
	topic text, -- Python Variables, Python Web Development, etc.
	question text, -- What is the popular web framework called?
	opta text, -- Flask
	optb text, -- Tumbrel
	optc text, -- Handi
	optd text, -- Kettle
	answer int, -- 1
	explanation text
);

-- insert default rows
