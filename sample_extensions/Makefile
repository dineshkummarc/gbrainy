all: subdirs

.PHONY: all subdirs clean
subdirs:
	${MAKE} -C Logic ${MAKECMDGOALS}
	${MAKE} -C Memory ${MAKECMDGOALS}
	${MAKE} -C Calculation  ${MAKECMDGOALS}

clean: subdirs


