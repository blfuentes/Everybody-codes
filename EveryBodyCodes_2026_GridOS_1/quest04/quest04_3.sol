HEADS AAA		// read head, and above and to the left counting heads

START	***	READR	***	SUL		// spread heads

READR	#**	READR	*@@	RRS		// reads each line, going right and left

READR	=-*	READR	***	RRS
READR	=_*	READR	*-*	RRS
READR	=1*	READR	***	RRS

READR	|_*	READL	*1*	LLS		// note above the shortest nail
READR	|-*	READL	*1*	LLS
READR	|1*	READL	***	LLS
READL	=**	READL	***	LLS
READL	#**	READR	***	DSD		// read next line
READR	_**	WRITEU3	***	USU		// all lines read

								// delete 1st head from top line
WRITEU3	#** WRITEU3	***	USU		// move up
WRITEU3	@**	WRITER3	***	DSD
WRITER3	#@@	WRITER3	***	RRS
WRITER3	*-*	WRITER3	***	RRS
WRITER3	*1*	WRITEL	*-*	LLS
WRITEL	*-*	WRITEL	***	LLS
WRITEL	#@@	WRITER	*_*	DSD

WRITER	***	WRITED	***	RRS		// prepare to change nails
WRITED	*-@	WRITED	***	DSD
WRITED	!1@	WRITED	|**	DSD
WRITED	_1@	WRITED	***	DSD

WRITED	_-_	WRITEU	***	USU		// write shaft
WRITEU	=-*	WRITEU	***	USU
WRITEU	|-@	WRITEU	***	USU
WRITEU	_-@	WRITEU	***	USU
WRITEU	_1@	WRITEU	***	USU
WRITEU	-**	WRITER	_**	DSD

WRITED	*1_	WRITEU	***	USU		// write head
WRITEU	|1*	WRITEU	***	USU
WRITEU	1**	WRITER2	_**	RRS		// clean up top line

WRITER2	*_*	DELETE	***	SSD		// clean up too much nail
WRITER2	*!*	WRITED2	***	DSD

WRITED2	*!@	WRITED2	_**	DSD
WRITED2	_!_	WRITEU2	***	USU
WRITEU2	_!@	WRITEU2	*** USU
WRITEU2	!!_	WRITER2	_**	RRS

DELETE	**@	DELETE	**_	SSD		// clean up left column
DELETE	**_	STOP	***	SSS